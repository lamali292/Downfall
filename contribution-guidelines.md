## Contribution Guidelines

### Steps
- Create your own git `fork` of this repository (Downfall).
- Work on a branch. When ready, open a PR to the default branch in the original repo.

### Code Guidelines
- Match the existing style and conventions in the codebase.
- Don't cram a bunch of unrelated fixes into one PR. Code is reviewed by hand, so keep it to one thing.
  - If you do bundle changes, split the changes into separate, logical commits, with a clear description of what each commit does.
- No vibe code: if you can't explain or defend what it does, don't PR it. 
  - Using Claude/ChatGPT as a Stack Overflow replacement or code explainer for debug issues is fine, just keep it reasonable. A huge AI god-class you don't understand will probably get rejected.
- Don't hardcode an abstract model in another abstract model. Use hooks and interfaces and create custom ones if AbstractModel doenst have any. There is a helper class `DownfallHook.cs`.
- 