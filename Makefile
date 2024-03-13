.ONESHELL:

BACKEND_REPOSITORY := amieldelatorre/weighttracker-backend
FRONTEND_REPOSITORY := amieldelatorre/weighttracker-frontend

# Frontend
build-frontend:
				echo "Building frontend with 'latest' tag..."
				docker build -t "${FRONTEND_REPOSITORY}:latest" .

push-frontend:
				echo "Pushing frontend with 'latest' tag..."
				docker buildx build --platform=linux/amd64,linux/arm64 --push -t "${FRONTEND_REPOSITORY}:latest" .

build-push-frontend: build-frontend push-frontend

# Backend
build-backend:
				echo "Building backend with 'latest' tag..."
				docker build -t "${BACKEND_REPOSITORY}:latest" .

push-backend:
				echo "Pushing backend with 'latest' tag..."
				docker buildx build --platform=linux/amd64,linux/arm64 --push -t "${BACKEND_REPOSITORY}:latest" .

build-push-backend: build-backend push-backend

# All
build-push-all: build-push-backend # build-push-frontend


# Terraform
tf-apply:
				cd ./infra
				. ./.env
				rm -rf .terraform .terraform.lock.hcl
				terraform init -backend-config=backend.conf
				terraform apply

				site_bucket=$$(terraform output -json | jq -r '.static_site_s3_bucket_name.value')
				aws s3 sync ../frontend/html "s3://$${site_bucket}" --delete
				# create cloudfront cache invalidation