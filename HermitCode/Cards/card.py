"""
HermitCardModel -> ConstructedCardModel transformer
====================================================
Per-file changes:
  - Namespace updated to BASE_NAMESPACE.{Rarity}
  - Usings cleaned (removes DynamicVar/HoverTips/ValueProps, adds BaseLib.Utils)
  - const fields removed
  - CanonicalVars       -> WithDamage/WithBlock/WithPower/WithCards/WithHeal in constructor
  - CanonicalKeywords   -> WithKeyword(...) in constructor  (property removed)
  - AdditionalHoverTips -> removed (WithPower auto-adds tips)
  - ExtraHoverTips      -> removed (WithPower auto-adds tips)
  - OnUpgrade           -> removed (upgrade values baked into With* calls)
  - DamageCmd.Attack(...).FromCard(this).Targeting(play.Target!) -> CommonActions.CardAttack(this, play)
  - PowerCmd.Apply<T>(ctx, Owner.Creature, DynamicVars[...], Owner.Creature, this) -> CommonActions.ApplySelf<T>(ctx, this)
  - Files already in the correct Rarity/ subfolder are updated in-place (not moved)
"""

import re
import sys
from pathlib import Path

# ── Config ────────────────────────────────────────────────────────────────────
SOURCE_DIR     = Path(".")
BASE_NAMESPACE = "Hermit.HermitCode.Cards"  # rarity subfolder appended automatically
DRY_RUN        = False                       # True = preview only, no writes
MOVE_TO_RARITY = True                        # move file into Rarity/ subfolder if not already there
# ─────────────────────────────────────────────────────────────────────────────

USINGS_REMOVE = {
    "MegaCrit.Sts2.Core.HoverTips",
    "MegaCrit.Sts2.Core.Localization.DynamicVars",
    "MegaCrit.Sts2.Core.ValueProps",
}
USINGS_ADD = ["BaseLib.Utils"]


# ── Helpers ───────────────────────────────────────────────────────────────────

def eval_expr(expr, consts):
    expr = expr.strip()
    for name, val in sorted(consts.items(), key=lambda x: -len(x[0])):
        expr = re.sub(r'\b' + re.escape(name) + r'\b', str(val), expr)
    try:
        return int(eval(expr))
    except Exception:
        return 0


def extract_brace_block(text, from_idx):
    i = text.index('{', from_idx)
    depth = 0
    for j in range(i, len(text)):
        if text[j] == '{':
            depth += 1
        elif text[j] == '}':
            depth -= 1
            if depth == 0:
                return i, j
    raise ValueError("Unmatched brace")


def find_member(text, sig_pattern):
    """Returns (decl_start, full_end, body_str) or None. Handles => and { } forms."""
    m = re.search(sig_pattern, text, re.DOTALL)
    if not m:
        return None
    rest_stripped = text[m.end():].lstrip()
    if rest_stripped.startswith('=>'):
        semi = text.index(';', m.end())
        return m.start(), semi + 1, text[m.end():semi]
    else:
        open_i, close_i = extract_brace_block(text, m.end())
        return m.start(), close_i + 1, text[open_i + 1:close_i]


def remove_member(text, sig_pattern):
    result = find_member(text, sig_pattern)
    if result is None:
        return text
    start, end, _ = result
    while start > 0 and text[start - 1] in (' ', '\t'):
        start -= 1
    if start > 0 and text[start - 1] == '\n':
        start -= 1
    return text[:start] + text[end:]


# ── Parsing ───────────────────────────────────────────────────────────────────

CAST_PREFIX = r'(?:\(\w+\))?'  # matches optional (decimal)/(int)/(Decimal) cast

def resolve_val(name, consts):
    return consts.get(name, name)

def parse_canonical_vars(block, consts):
    result = []
    for m in re.finditer(r'new DamageVar\(' + CAST_PREFIX + r'(\w+)(?:,[^)]+)?\)', block):
        result.append(('Damage', resolve_val(m.group(1), consts)))
    for m in re.finditer(r'new BlockVar\(' + CAST_PREFIX + r'(\w+)(?:,[^)]+)?\)', block):
        result.append(('Block', resolve_val(m.group(1), consts)))
    for m in re.finditer(r'new PowerVar<(\w+)>\(' + CAST_PREFIX + r'(\w+)\)', block):
        result.append(('Power', resolve_val(m.group(2), consts), m.group(1)))
    for m in re.finditer(r'new CardsVar\(' + CAST_PREFIX + r'(\w+)\)', block):
        result.append(('Cards', resolve_val(m.group(1), consts)))
    for m in re.finditer(r'new HealVar\(' + CAST_PREFIX + r'(\w+)\)', block):
        result.append(('Heal', resolve_val(m.group(1), consts)))
    return result


def parse_canonical_keywords(block):
    return re.findall(r'CardKeyword\.(\w+)', block)


def parse_canonical_tags(block):
    return re.findall(r'CardTag\.(\w+)', block)


def parse_upgrades(body, consts):
    """Returns (upgrades_dict, keyword_upgrades_list).
    keyword_upgrades_list entries: (CardKeyword name, 'Add'|'Remove')
    """
    u = {}
    for m in re.finditer(r'DynamicVars\.Damage\.UpgradeValueBy\(([^)]+)\)', body):
        u['Damage'] = eval_expr(m.group(1), consts)
    for m in re.finditer(r'DynamicVars\.Block\.UpgradeValueBy\(([^)]+)\)', body):
        u['Block'] = eval_expr(m.group(1), consts)
    for m in re.finditer(r'DynamicVars\.Cards\.UpgradeValueBy\(([^)]+)\)', body):
        u['Cards'] = eval_expr(m.group(1), consts)
    for m in re.finditer(r'DynamicVars\["([^"]+)"\]\.UpgradeValueBy\(([^)]+)\)', body):
        u[m.group(1)] = eval_expr(m.group(2), consts)

    kw_upgrades = []
    for m in re.finditer(r'(?:this\.)?AddKeyword\(CardKeyword\.(\w+)\)', body):
        kw_upgrades.append((m.group(1), 'Add'))
    for m in re.finditer(r'(?:this\.)?RemoveKeyword\(CardKeyword\.(\w+)\)', body):
        kw_upgrades.append((m.group(1), 'Remove'))

    return u, kw_upgrades


def strip_handled_upgrade_lines(body, consts):
    """Remove lines from OnUpgrade body that have been migrated to With* calls.
    Returns cleaned body, or None if nothing meaningful remains."""
    handled_patterns = [
        r'DynamicVars\.Damage\.UpgradeValueBy\([^)]+\);',
        r'DynamicVars\.Block\.UpgradeValueBy\([^)]+\);',
        r'DynamicVars\.Cards\.UpgradeValueBy\([^)]+\);',
        r'DynamicVars\["[^"]+"\]\.UpgradeValueBy\([^)]+\);',
        r'EnergyCost\.UpgradeBy\([^)]+\);',
        r'(?:this\.)?AddKeyword\(CardKeyword\.\w+\);',
        r'(?:this\.)?RemoveKeyword\(CardKeyword\.\w+\);',
    ]
    cleaned = body
    for pat in handled_patterns:
        cleaned = re.sub(r'[ \t]*' + pat + r'[ \t]*\n?', '', cleaned)
    return None if not cleaned.strip() else cleaned


def build_with_calls(vars_list, upgrades, cost_upgrade, keywords, tags, kw_upgrades=None):
    lines = []
    for v in vars_list:
        kind = v[0]
        if kind == 'Damage':
            lines.append(f'        WithDamage({v[1]}, {upgrades.get("Damage", 0)});')
        elif kind == 'Block':
            lines.append(f'        WithBlock({v[1]}, {upgrades.get("Block", 0)});')
        elif kind == 'Power':
            lines.append(f'        WithPower<{v[2]}>({v[1]}, {upgrades.get(v[2], 0)});')
        elif kind == 'Cards':
            lines.append(f'        WithCards({v[1]}, {upgrades.get("Cards", 0)});')
        elif kind == 'Heal':
            lines.append(f'        WithHeal({v[1]}, {upgrades.get("Heal", 0)});')
    for kw in keywords:
        lines.append(f'        WithKeyword(CardKeyword.{kw});')
    if tags:
        tag_args = ', '.join('CardTag.' + t for t in tags)
        lines.append('        WithTags(' + tag_args + ');')
    for kw, upg_type in (kw_upgrades or []):
        lines.append('        WithKeyword(CardKeyword.' + kw + ', UpgradeType.' + upg_type + ');')
    if cost_upgrade is not None:
        lines.append('        WithCostUpgradeBy(' + str(cost_upgrade) + ');')
    return lines


# ── Usings ────────────────────────────────────────────────────────────────────

# Map from using namespace to identifier tokens that indicate it's still needed
USING_TOKENS = {
    'MegaCrit.Sts2.Core.HoverTips':               ['IHoverTip', 'HoverTipFactory'],
    'MegaCrit.Sts2.Core.Localization.DynamicVars': ['DynamicVar', 'BlockVar', 'DamageVar',
                                                     'PowerVar', 'CardsVar', 'HealVar',
                                                     'EnergyVar', 'HealVar'],
    'MegaCrit.Sts2.Core.ValueProps':               ['ValueProp'],
}

def update_usings(content):
    for u in USINGS_REMOVE:
        tokens = USING_TOKENS.get(u, [])
        still_used = any(
            # check for token outside of the using line itself
            re.search(r'\b' + t + r'\b',
                      re.sub(r'using [^\n]+;\n', '', content))
            for t in tokens
        ) if tokens else False
        if not still_used:
            content = re.sub(rf'[ \t]*using {re.escape(u)};\n', '', content)
    for u in USINGS_ADD:
        if f'using {u};' not in content:
            last = None
            for m in re.finditer(r'using [^\n]+;\n', content):
                last = m
            if last:
                pos = last.end()
                content = content[:pos] + f'using {u};\n' + content[pos:]
    return content


# ── Attack chain transform ────────────────────────────────────────────────────

def transform_attack_chain(content):
    pattern = (
        r'DamageCmd\.Attack\([^)]+\)'
        r'(?:\s*\n?\s*\.\w+\([^)]*\))*'
        r'\s*\n?\s*\.FromCard\(this\)'
        r'\s*\n?\s*\.Targeting\(play\.Target!?\)'
        r'((?:\s*\n?\s*\.\w+\([^)]*\))*)'
        r'\s*\n?\s*\.Execute\(ctx\)'
    )

    def replacer(m):
        extra = m.group(1)
        if extra.strip():
            return f'CommonActions.CardAttack(this, play){extra}\n            .Execute(ctx)'
        return 'CommonActions.CardAttack(this, play).Execute(ctx)'

    return re.sub(pattern, replacer, content, flags=re.DOTALL)


# ── PowerCmd.Apply self-apply transform ───────────────────────────────────────

def transform_power_apply(content):
    return re.sub(
        r'await PowerCmd\.Apply<(\w+)>\(\s*'
        r'ctx,\s*Owner\.Creature,\s*'
        r'DynamicVars\["[^"]+"\]\.IntValue,\s*'
        r'Owner\.Creature,\s*this\s*\);',
        r'await CommonActions.ApplySelf<\1>(ctx, this);',
        content
    )


# ── Primary constructor transform ────────────────────────────────────────────

def transform_primary_constructor(content):
    """
    Convert primary constructor syntax to explicit constructor:
      public class Foo() : Base(args)  {  ...  }
    ->
      public class Foo : Base
      {
          public Foo() : base(args) { ... }
      }
    """
    pattern = (
        r'(public(?:\s+sealed|\s+abstract)?\s+class\s+(\w+))'  # group1=modifiers+class+name, group2=name
        r'\s*\(\s*\)'                                            # primary ctor ()
        r'\s*:\s*(\w+)\s*\(([^)]*)\)'                          # : BaseClass(args)  group3=base, group4=args
    )
    m = re.search(pattern, content)
    if not m:
        return content

    class_name = m.group(2)
    base_args  = m.group(4).strip()
    decl_end   = m.end()

    # Find the class body braces
    open_i, close_i = extract_brace_block(content, decl_end)
    class_body = content[open_i + 1:close_i]

    # Build replacement
    nl = '\n'
    new_decl  = m.group(1) + ' : HermitCardModel'
    new_ctor  = '    public ' + class_name + '() : base(' + base_args + ')' + nl + '    {' + nl + '    }'
    new_class = new_decl + nl + '{' + nl + new_ctor + nl + class_body + '}'

    return content[:m.start()] + new_class + content[close_i + 1:]


# ── Main transform ────────────────────────────────────────────────────────────

def transform(content):
    # 1. Parse const fields
    consts = {
        m.group(1): int(m.group(2))
        for m in re.finditer(r'private const \w+ (\w+) = (-?\d+);', content)
    }

    # 2. Rarity
    rm = re.search(r'CardRarity\.(\w+)', content)
    rarity = rm.group(1) if rm else 'Common'

    # 3. CanonicalVars
    cv = find_member(content, r'protected override IEnumerable<DynamicVar> CanonicalVars')
    vars_list = parse_canonical_vars(cv[2], consts) if cv else []

    # 4. OnUpgrade
    ou = find_member(content, r'protected override void OnUpgrade\(\)')
    upgrades, kw_upgrades = parse_upgrades(ou[2], consts) if ou else ({}, [])
    cost_upgrade = None
    if ou:
        cm = re.search(r'EnergyCost\.UpgradeBy\((-?\d+)\)', ou[2])
        if cm:
            cost_upgrade = int(cm.group(1))

    # 5. CanonicalKeywords
    ck = find_member(content, r'public override IEnumerable<CardKeyword> CanonicalKeywords')
    keywords = parse_canonical_keywords(ck[2]) if ck else []

    # 6. CanonicalTags
    ct = find_member(content, r'protected override HashSet<CardTag> CanonicalTags')
    tags = parse_canonical_tags(ct[2]) if ct else []

    # 7. Build With* constructor calls
    with_calls = build_with_calls(vars_list, upgrades, cost_upgrade, keywords, tags, kw_upgrades)

    # ── Mutations ─────────────────────────────────────────────────────────────

    changes = []

    prev = content
    content = transform_primary_constructor(content)
    if content != prev: changes.append('primary constructor → explicit constructor + base(...)')

    prev = content
    content = re.sub(r'namespace\s+\S+;', f'namespace {BASE_NAMESPACE}.{rarity};', content)
    if content != prev: changes.append(f'namespace → {BASE_NAMESPACE}.{rarity}')
    prev = content
    content = update_usings(content)
    if content != prev: changes.append('usings updated')
    # Remove const fields only if the name no longer appears anywhere else in the file
    def remove_unused_consts(text):
        lines = text.split('\n')
        result = []
        for line in lines:
            m = re.match(r'[ \t]*private const \w+ (\w+) = -?\d+;', line)
            if m:
                name = m.group(1)
                # Check if the name appears anywhere outside of this declaration line
                rest = text.replace(line, '', 1)
                if re.search(r'\b' + re.escape(name) + r'\b', rest):
                    result.append(line)  # still referenced — keep it
                # else: drop the line
            else:
                result.append(line)
        return '\n'.join(result)

    prev = content
    content = remove_unused_consts(content)
    if content != prev: changes.append('unused const fields removed')

    # Only remove CanonicalVars if every `new *Var(` entry was recognised
    if cv:
        total_var_entries = len(re.findall(r'new \w+Var\(', cv[2]))
        if total_var_entries > 0 and len(vars_list) == total_var_entries:
            prev = content
            content = remove_member(content, r'protected override IEnumerable<DynamicVar> CanonicalVars')
            if content != prev: changes.append('CanonicalVars removed → With* calls in constructor')
            prev = content
            content = remove_member(content, r'protected override IEnumerable<IHoverTip> AdditionalHoverTips')
            content = remove_member(content, r'protected override IEnumerable<IHoverTip> ExtraHoverTips')
            if content != prev: changes.append('AdditionalHoverTips/ExtraHoverTips removed (covered by WithPower)')
        elif cv and total_var_entries == 0:
            content = remove_member(content, r'protected override IEnumerable<DynamicVar> CanonicalVars')
    else:
        # No CanonicalVars — but still remove tip overrides if they only have FromPower (covered by WithPower)
        def _only_from_power(block):
            stripped = re.sub(r'HoverTipFactory\.FromPower<\w+>\(\)', '', block)
            stripped = re.sub(r'[\[\],\s]', '', stripped)
            return stripped.strip() == '=>'
        for tip_sig in (
            r'protected override IEnumerable<IHoverTip> AdditionalHoverTips',
            r'protected override IEnumerable<IHoverTip> ExtraHoverTips',
        ):
            tip_member = find_member(content, tip_sig)
            if tip_member and _only_from_power(tip_member[2]):
                content = remove_member(content, tip_sig)

    # Only remove CanonicalKeywords if all keywords were migrated
    if ck and keywords:
        prev = content
        content = remove_member(content, r'public override IEnumerable<CardKeyword> CanonicalKeywords')
        if content != prev: changes.append('CanonicalKeywords removed → WithKeyword(...) in constructor')

    if ct and tags:
        prev = content
        content = remove_member(content, r'protected override HashSet<CardTag> CanonicalTags')
        if content != prev: changes.append('CanonicalTags removed → WithTags(...) in constructor')
    if ou:
        cleaned_body = strip_handled_upgrade_lines(ou[2], consts)
        if cleaned_body is None:
            content = remove_member(content, r'protected override void OnUpgrade\(\)')
            changes.append('OnUpgrade removed (all logic migrated to constructor)')
        else:
            result = find_member(content, r'protected override void OnUpgrade\(\)')
            if result:
                open_i = content.index('{', result[0])
                close_i = result[1] - 1
                prev = content
                content = content[:open_i + 1] + cleaned_body + content[close_i:]
                if content != prev: changes.append('OnUpgrade: migrated lines stripped, remainder kept')

    # Inject With* calls into constructor
    if with_calls:
        changes.append('constructor: ' + ', '.join(
            re.sub(r'\s+', ' ', c.strip().rstrip(';')) for c in with_calls
        ))
        inject = '\n' + '\n'.join(with_calls)
        # `: base(...) { }` same-line empty constructor
        m = re.search(r'(: base\([^)]+\))\s*\{\s*\}', content)
        if m:
            content = content[:m.start()] + m.group(1) + '\n    {' + inject + '\n    }' + content[m.end():]
        else:
            # Block form - just insert inject lines right after the opening {
            m = re.search(r': base\([^)]+\)', content)
            if m:
                brace_pos = content.index('{', m.end())
                content = content[:brace_pos + 1] + inject + '\n' + content[brace_pos + 1:]

    prev = content
    content = transform_attack_chain(content)
    if content != prev: changes.append('DamageCmd.Attack chain → CommonActions.CardAttack')
    prev = content
    content = transform_power_apply(content)
    if content != prev: changes.append('PowerCmd.Apply self → CommonActions.ApplySelf')
    content = re.sub(r'\n{3,}', '\n\n', content)

    return content, changes


def _chk(before, after, label, changes):
    if before != after:
        changes.append(label)


# ── Entry point ───────────────────────────────────────────────────────────────

def main():
    files = list(SOURCE_DIR.rglob("*.cs"))
    print(f"Found {len(files)} .cs files\n")

    transformed = 0
    skipped = 0

    for path in files:
        src = path.read_text(encoding='utf-8')

        if 'HermitCardModel' not in src:
            continue
        needs_transform = any(k in src for k in (
            'CanonicalVars', 'OnUpgrade', 'CanonicalKeywords',
            'AdditionalHoverTips', 'ExtraHoverTips', 'CanonicalTags',
        )) or re.search(r'class \w+\(\)', src)
        if not needs_transform:
            skipped += 1
            print(f"  [skip]  {path.name}  (nothing to transform)")
            continue

        result, changes = transform(src)

        if result == src:
            skipped += 1
            print(f"  [same]  {path.name}")
            continue

        rm = re.search(r'CardRarity\.(\w+)', src)
        rarity = rm.group(1) if rm else None
        transformed += 1

        if DRY_RUN:
            action = 'in-place' if (rarity and path.parent.name == rarity) else f'-> {rarity}/'
            print(f"  [dry]   {path.name}  {action}")
            continue

        # Move only if not already in the correct rarity subfolder
        comment_lines = ['', '/* transform_cards.py changes:']
        for c in changes:
            comment_lines.append(' *   ' + c)
        comment_lines.append(' */')
        result = result.rstrip() + '\n' + '\n'.join(comment_lines) + '\n'

        if MOVE_TO_RARITY and rarity and path.parent.name != rarity:
            dest = path.parent / rarity / path.name
            dest.parent.mkdir(exist_ok=True)
            dest.write_text(result, encoding='utf-8')
            path.unlink()
            print(f"  [done]  {path.name}  ->  {rarity}/")
        else:
            path.write_text(result, encoding='utf-8')
            print(f"  [done]  {path.name}  (in-place)")

    print(f"\nTransformed: {transformed}  |  Skipped: {skipped}")


if __name__ == '__main__':
    if '--dry-run' in sys.argv:
        DRY_RUN = True
    main()