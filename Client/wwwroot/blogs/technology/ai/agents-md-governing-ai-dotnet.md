# `agents.md`: Governing AI in Real-World Codebases  
*A practical guide for teams using Codex and agentic coding tools*

---

## Introduction: Why `agents.md` Exists

As AI-assisted coding tools like **Codex CLI** become part of everyday development, teams quickly run into a problem:

> AI is fast — but speed without guardrails is dangerous.

Left unconstrained, AI tools tend to:
- Over-refactor
- Touch files they shouldn’t
- Optimize for “task completion” instead of long-term maintainability

This is where **`agents.md`** comes in.

`agents.md` is a **repository-level governance file** that defines *how an AI agent is allowed to behave inside your codebase*.  
Think of it as **engineering culture, written down, and enforced automatically**.

This blog explains what `agents.md` is, how it works, and how to use it effectively at **team and organization scale**, especially for **.NET, Blazor, ASP.NET Core, and gRPC** projects.

---

## Part 1: What Is `agents.md`?

### The Core Idea

`agents.md` is a Markdown file placed at the **root of a repository**.  
Codex automatically reads it before performing any action.

Its purpose is to answer five questions for the AI:

1. Who am I in this repository?
2. What am I allowed to change?
3. What must I never touch?
4. What workflow must I follow?
5. When should I stop and ask a human?

In practice, this turns Codex from *“AI autocomplete”* into a **disciplined junior engineer**.

---

## Part 2: How Codex Uses `agents.md`

![agents.md governance flow](/images/agents-md-governance-flow.svg)


Codex treats `agents.md` as **higher priority than your prompt**.

If your prompt conflicts with rules in `agents.md`, the correct behavior is:
- Stop
- Explain the conflict
- Ask for clarification

This is critical for real projects.

```
repo-root/
├─ agents.md   ← always read by Codex
├─ Pages/
├─ Components/
├─ wwwroot/
└─ ...
```

---

## Part 3: Anatomy of a Production-Grade `agents.md`

A strong `agents.md` is **explicit, concise, and enforceable**.

### 1. Role Definition

```md
You are a senior .NET engineer specializing in Blazor and ASP.NET Core.
```

### 2. Non-Negotiable Rules

```md
- Do NOT modify Program.cs
- Do NOT modify *.csproj files
- Do NOT introduce new frameworks or dependencies
```

### 3. File Scope Rules

```md
You MAY modify:
- Pages/
- Components/
- wwwroot/

You MUST NOT modify:
- Program.cs
- appsettings*.json
```

### 4. Workflow Rules

![Plan → Confirm → Apply workflow](/images/plan-confirm-apply.svg)


```md
Workflow:
1. Explain the approach
2. List files to be modified
3. WAIT for confirmation
4. Apply changes
5. Summarize what was done
```

---

## Part 4: Team-Scale Usage

![Team-scale agents.md architecture](/images/team-scale-agents-md.svg)


At team scale, `agents.md` should be:
- Centrally defined
- Locally specialized
- Automatically validated

This avoids configuration drift across repositories.

---

## Part 5: Pairing `agents.md` with Git Hooks and CI

- Git hooks prevent missing or invalid `agents.md`
- CI checks enforce compliance
- Secret scanning adds a safety net

AI rules should not be optional.

---

## Part 6: Role-Based Agent Patterns

Separate responsibilities explicitly:
- Reviewer (no file changes)
- Implementer (plan → confirm → apply)
- Tester (tests only)

This mirrors healthy team workflows.

---

## Part 7: Context Management

Context is finite.

When Codex reports high remaining context, you're safe.  
As context fills, risk increases.

Best practice:
- Work in phases
- Restart sessions deliberately
- Keep `agents.md` concise

---

## Conclusion

`agents.md` is not about limiting AI — it is about **scaling engineering discipline**.

It encodes:
- Standards
- Safety
- Workflow
- Culture

If you wouldn’t want a new junior developer to do something, Codex shouldn’t either.

---

## References

- https://learn.microsoft.com/dotnet/
- https://learn.microsoft.com/aspnet/core
- https://code.visualstudio.com/docs/remote/wsl
- https://www.promptingguide.ai/
- https://git-scm.com/book/en/v2
