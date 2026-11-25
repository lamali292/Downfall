"""Rate-limited Paratranz HTTP client shared by para_download.py, build/loc/para.py and build/loc/para_sync.py.

Three layers, because a plain concurrency cap is not enough for Paratranz:
  * start-rate limit: request *starts* are spaced `interval` seconds apart across all threads;
  * concurrency cap: at most `workers` requests in flight;
  * shared cooldown: when any thread gets a 429, every thread pauses (Retry-After, else exponential),
    and the start interval is doubled for the rest of the run (adaptive backoff, max MAX_INTERVAL).
"""
import threading
import time

import requests

MAX_INTERVAL = 2.0
RETRIES = 8


class Client:
    def __init__(self, workers=3, interval=0.25):
        self.session = requests.Session()  # keep-alive
        self.slots = threading.BoundedSemaphore(max(1, workers))
        self.interval = interval
        self._lock = threading.Lock()
        self._next_start = 0.0   # earliest time the next request may start
        self._streak = 0         # consecutive 429s, for exponential fallback

    def configure(self, workers=None, interval=None):
        if workers is not None:
            self.slots = threading.BoundedSemaphore(max(1, workers))
        if interval is not None:
            self.interval = interval

    def _reserve_start(self):
        """Block until this request's start slot, then return."""
        with self._lock:
            now = time.monotonic()
            start = max(now, self._next_start)
            self._next_start = start + self.interval
        if start > now:
            time.sleep(start - now)

    def _cooldown(self, seconds):
        with self._lock:
            self._next_start = max(self._next_start, time.monotonic() + seconds)
            self.interval = min(MAX_INTERVAL, self.interval * 2)  # stay slower for the rest of the run

    def request(self, method, url, **kwargs):
        """Like requests.request, with limiting + retry on 429/5xx/network errors. Returns the final response
        (the caller checks .ok / raise_for_status)."""
        kwargs.setdefault("timeout", 120)
        for attempt in range(RETRIES):
            last = attempt == RETRIES - 1
            self._reserve_start()
            try:
                with self.slots:
                    # Uploads pass open file handles; rewind so a retry re-sends the whole file.
                    for fobj in (kwargs.get("files") or {}).values():
                        fobj[1].seek(0)
                    r = self.session.request(method, url, **kwargs)
            except requests.RequestException:
                if last:
                    raise
                time.sleep(2 ** min(attempt, 5))
                continue
            if r.status_code == 429 or r.status_code >= 500:
                if last:
                    return r
                try:
                    wait = float(r.headers.get("Retry-After", ""))
                except ValueError:
                    self._streak += 1
                    wait = min(60, 2 ** self._streak)
                print(f"  rate limited ({r.status_code}); pausing all requests {wait:.0f}s")
                self._cooldown(wait)
                continue
            self._streak = 0
            return r


client = Client()
