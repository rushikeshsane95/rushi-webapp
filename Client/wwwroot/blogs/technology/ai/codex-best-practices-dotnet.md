# Using Codex CLI Effectively in Real-World .NET Projects  

*A practical, opinionated guide for professional .NET developers*

---

## Foundations – Why Codex Matters for .NET Developers

### Introduction

Codex CLI is not “AI that writes code for you.”  
Used correctly, it is a **developer accelerator**: a tool that helps you reason about code, automate repetitive changes, and explore solutions faster — while *you* remain in control.

For .NET developers working with **ASP.NET Core, Blazor, Web APIs, and gRPC**, Codex shines when combined with:
- A modern Linux-based dev environment (WSL on Windows)
- VS Code with Remote WSL
- Codex CLI
- Strong repository-level guardrails (`agents.md`)
- Disciplined workflows (plan → confirm → apply)

This blog summarizes **best practices learned from real usage**, not theory — including what works well and what you *should* add to make Codex safer and more effective.

---

## Setting Up Codex in a Modern .NET Environment

### Recommended Environment

A proven setup for .NET + Codex looks like this:

```
Windows
 └─ WSL (Ubuntu)
     ├─ .NET SDK
     ├─ Node.js
     ├─ Codex CLI
     └─ Git
```

### Why WSL Matters

Running Codex *inside* WSL gives you:
- Linux-native tooling
- Faster file I/O
- Fewer path and permission issues
- Parity with CI and production containers

**Best practice**
- Keep repositories in `~/code`
- Install tooling inside WSL, not Windows

---

## Prompt Design – The Single Biggest Quality Lever

### Bad Prompt

> “Refactor this Blazor project to be cleaner.”

### Good Prompt

```
Follow agents.md.
Explain the plan.
List files.
Wait for confirmation.
Task: Extract a reusable Blazor component.
```

Prompt quality matters more than model choice.

---

## Using agents.md Effectively

`agents.md` acts as governance for Codex:
- Defines allowed scope
- Prevents unsafe changes
- Enforces workflow discipline

Example:

```
Do NOT modify Program.cs.
Explain plan first.
Wait for approval.
```

---

## Context Management

When Codex shows “94% context left”, it indicates available working memory.

Best practice:
- Work in phases
- Restart sessions when context drops too low
- Avoid scanning entire repos unnecessarily

---

## Safe Workflows

Use Codex like a junior engineer:
1. Ask for understanding
2. Ask for a plan
3. Confirm
4. Apply
5. Review via Git

---

## Common Mistakes

- Treating Codex like chat
- Letting it refactor broadly
- Ignoring Git hygiene
- Overly verbose prompts

---

## Practical Takeaways

- Codex amplifies good engineering practices
- Governance is essential
- Context is a finite resource
- Small changes beat big refactors

---

## References

- https://learn.microsoft.com/dotnet/
- https://code.visualstudio.com/docs/remote/wsl
- https://learn.microsoft.com/windows/wsl/
- https://www.promptingguide.ai/
