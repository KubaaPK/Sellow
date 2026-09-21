# Contributing

Sellow is developed as a solo learning and portfolio project.
This workflow keeps changes focused, reviewable, and documented.

## Workflow

1. Create an issue describing the goal, scope, and acceptance criteria.
2. Create a short-lived branch for the task.
3. Discuss the approach before implementing complex changes.
4. Implement the change and verify its behavior.
5. Open a pull request and link the issue.
6. Resolve blocking review findings.
7. Merge the pull request once the definition of done is satisfied.

Use `Closes #<issue-number>` in the pull request description when
merging it should close the issue.

## Branches and Commits

Use descriptive branch names, for example:

- `docs/contribution-guidelines`
- `feat/create-offer`
- `fix/order-quantity-validation`

Write concise commit messages describing the change, for example:

- `docs: add contribution guidelines`
- `feat: allow sellers to create offers`
- `fix: reject orders exceeding available stock`

## Pull Requests

Describe:

- What changed and why.
- How the change was verified.
- Any known limitations or unresolved questions.

Keep each pull request focused on one coherent task.

## Definition of Done

A task is complete when:

- Its acceptance criteria are satisfied.
- The change has been verified and the verification is described in the PR.
- Appropriate tests have been added or updated where needed.
- Relevant documentation has been updated.
- Blocking review findings have been resolved.
- The developer can explain the solution and its main trade-offs.

Documentation-only changes do not require automated tests.
Optional review suggestions do not block completion.

## Language

Use English for documentation, issues, pull requests, and commit messages.

## AI Assistance

AI is used for mentoring and code review, and may provide examples
or implementation assistance when requested.

The developer remains responsible for reviewing, verifying, and
understanding all submitted changes.
