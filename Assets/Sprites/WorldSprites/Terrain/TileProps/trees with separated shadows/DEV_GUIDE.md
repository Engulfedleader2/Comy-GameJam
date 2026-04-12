# Developer Guide

This document contains development notes and workflow guidelines for contributors.

This is a collaborative game jam project. 

---

## Engine

Engine: **Godot 4.6.1**

---

## Languages

Primary language:

- **C#**

Secondary scripting language:

- **GDScript**

---

## Branching Workflow

All work should be done in a **separate branch**.  
Do **not** commit directly to `main` or `staging`

Each **feature, bug fix, or improvement** should have its own branch.

Example branch names: Feature/PlayerAnimations, feature/options-menu, fix/jump-bug

---

## Pull Request Workflow

All changes should be submitted through a **Pull Request (PR)**.

Typical workflow:

1. Pull the latest changes from `main`
2. Create a new feature branch
3. Make your changes
4. Push the branch to GitHub
5. Open a Pull Request into staging
6. Request a code review before merging

---

## Commit Message Guidelines

Keep commit messages clear and descriptive.

Good examples:

Add main menu UI  
Implement player jump logic  
Fix animation transition bug  
Adjust UI button spacing  

Avoid vague commit messages like:

update  
fix  
stuff  
