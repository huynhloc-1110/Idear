# Working process on Git and GitHub
The project will have 2 main branches and some sub-branches:

## Sub-branches
- These are the branches that we create when working on a specific feature.

> For example, if Huy is assigned to work on the login page for the project, he
will create a new branch called `feature-login` and work on that branch.

## `dev` branch
- This represents the current version of the project that everyone is working
on. A sub-branch, when completed, will be merged into the `dev` branch. Branch
merging must be done via pull request.

> For example, Huy has completed the `feature-login` branch and wants to merge
it into the `dev` branch. Huy goes to GitHub and creates a pull request for
`feature-login` to merge into `dev`. The pull request must be reviewed by at
least one other person (e.g., Tri) before it can be merged into `dev`.

- Note that after `dev` is merged with `feature-login`, other sub-branches must
be merged back from `dev`. This ensures that other sub-branches are updated with
the latest changes from dev.

> For example, the story continues after Huy has successfully merged
`feature-login` into `dev`. At this point, Khoa is also working on a sub-branch
called `feature-purchase`. Khoa notices that Huy's pull request has been
successfully merged into dev. Therefore, Khoa needs to update his branch from
dev. Khoa proceeds to merge dev into feature-purchase and resolves any conflicts
if there are any.

## `main` branch
- This is the default branch of GitHub, meaning that people who visit GitHub
repos will see this branch first, and when they clone it, they will see the
files from this branch first. Therefore, this branch is used for official
version releases. This branch will only be merged from `dev` branch after all
the features of a sprint have been completed.

> For example, this sprint includes 2 features: `feature-login` and
`feature-purchase`. Khoa has also completed `feature-purchase` and successfully
merged it into `dev`. At this point, `dev` has completed one sprint and will be
merged into `main`.

# Necessary git commands
- We get the project to our local machine using the command:
```shell
    git clone <repo_ssh_url>
```

- Create a new branch and go to that branch:
```shell
    git checkout -b <branch_name>
```

- Delete a branch (only used when creating a branch incorrectly or deleting a
completed sub-branch at local):
```shell
    git checkout <other_branch>         // go to another branch
    git branch -d <branch_to_delete>    // delete the branch to delete
```

- Push a branch to GitHub:
```shell
    git push origin <branch_name>
```

- Pull a sub-branch of someone else:
```shell
    git checkout <sub-branch_to_pull>
    git pull origin <sub-branch_to_pull>
```

- Merge a branch into another branch (only used when merging `dev` branch into
the current sub-branch; to merge a sub-branch into the `dev` branch, go to
GitHub and create a pull request):
```shell
    git checkout dev                // go to `dev`
    git pull origin dev             // pull the new `dev` to `dev`
    git checkout <feature_branch>   // go to the sub-branch
    git merge dev                   // merge into the `dev` branch
```

## Note
The keyword `origin` refers to our remote repos on GitHub. In the example below,
there are 2 branches locally: `develop` and `main`, with `develop` being our
current location. On GitHub, there are 3 branches: `develop`,
`feature-register`, and `main`, with `main` being the default branch on GitHub.
```console
* develop
  main
  remotes/origin/HEAD -> origin/main
  remotes/origin/develop
  remotes/origin/feature-register
  remotes/origin/main
```
As seen, `feature-register` is on remote but has not been brought to the local
yet. To fetch it to review (or do something else), use the following commands:
```shell
    git checkout feature-register
    git pull origin feature-register
```

# References
- Watch [this video](https://www.youtube.com/watch?v=1SXpE08hvGs) to understand
the GitFlow process. Note: our project does not have `release` and `hotfix`
branches to avoid complexity.