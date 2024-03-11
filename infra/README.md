# Infrastructure

## Initialising
```bash
# backend.conf example
region         = "<region>"
bucket         = "<bucket>"
key            = "<key>"
dynamodb_table = "<dynamodb-table>"
profile        = "<profile>"
encrypt        = false
```

```bash
# Environment variables
export TF_VAR_region="<region>"
export TF_VAR_aws_profile="<profile>"
```

```bash
# Initialise terraform
$ terraform init -backend-config=backend.conf
```