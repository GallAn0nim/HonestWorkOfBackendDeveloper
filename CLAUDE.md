# Code Style Guidelines

Write clean, natural, human-readable code that looks like it was written by an experienced senior developer.

- **KISS:** prefer straightforward solutions over unnecessary complexity.
- **DRY:** avoid duplicated business logic, but don't introduce abstractions for trivial repetition.
- **YAGNI:** implement only what is currently required. Never build for hypothetical future needs.
- **SOLID:** apply pragmatically, not dogmatically. Avoid excessive interfaces, inheritance, and abstraction layers.
- **Separation of Concerns:** keep responsibilities clearly separated without overengineering.
- **Single Responsibility:** each class and method has one clear purpose.
- **Clean Code:** readability, maintainability, and predictable behavior over clever implementations.
- **Meaningful Naming:** concise, descriptive, domain-specific names. No unnecessary abbreviations or overly verbose identifiers.
- **No Comments:** never write inline comments, block comments, XML documentation, or explanatory annotations. Code must be self-documenting.
- **Small, Focused Methods:** short methods with clear intent, without splitting simple logic into unnecessary helpers.
- **Early Returns:** use guard clauses to reduce nesting.
- **Always Braces:** every `if`, `else`, `for`, `foreach` and `while` body goes in braces on its own lines, even a single statement. One statement per line. C#: braces on their own line (enforced by `api/.editorconfig`). TypeScript: opening brace on the same line.
- **Explicit Over Clever:** no magic values, hidden side effects, unnecessary metaprogramming, or overly complex expressions.
- **Composition Over Inheritance:** when it genuinely reduces coupling and complexity.
- **Consistency:** follow existing project conventions, naming, formatting, patterns, and architecture.
- **Modern Practices:** idiomatic language features and established best practices without sacrificing clarity.
- **Error Handling:** handle realistic failures explicitly. Never silently swallow exceptions or add excessive defensive checks.
- **Minimal Changes:** modify only what is necessary. No unrelated refactoring, speculative improvements, or unnecessary dependencies.
- **Testability:** naturally testable code, without abstractions introduced solely for mocking.
- **Performance:** avoid obvious inefficiencies, but never sacrifice readability for unmeasured micro-optimizations.
- **No AI Fingerprints:** no boilerplate, redundant wrappers, unnecessary patterns, artificial complexity, or generated-looking structures.

# Observability

- Never log IP addresses, typed usernames or user agents into the public log stream. Guarded by `Log_stream_keeps_visitor_details_out` in `ObservabilityTests`.

Golden rule: the best code is the simplest code that correctly solves the problem, explains itself without comments, and remains easy to maintain.
