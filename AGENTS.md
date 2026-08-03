# Agent Instructions

## Repository validation

Use the terminal to inspect and validate changes rather than only describing
commands.

Before changing code:

1. Inspect `package.json` and the files under `.github/workflows`.
2. List available npm scripts with `npm run`.
3. Use script names exactly as defined; names are case-sensitive.
4. Install project dependencies using the repository's configured package
   manager.
5. Run the relevant lint, test and build commands after making changes.

For this repository, the primary frontend checks are:

    npm run lint
    npm run test-ci
    npm run build

When a command fails, inspect its complete output and determine whether the
failure is caused by:

- missing project dependencies;
- a missing executable;
- incorrect command syntax;
- environment configuration;
- or a genuine source or test failure.

Do not claim success unless the validation commands actually complete
successfully.

## Safety

Do not:

- expose credentials or environment variables;
- modify secrets;
- discard unrelated Git changes;
- use destructive Git commands;
- weaken tests merely to make them pass;
- execute code outside the repository unless required for normal validation.