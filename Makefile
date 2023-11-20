BACKEND_REPOSITORY := amieldelatorre/weighttracker-backend
FRONTEND_REPOSITORY := amieldelatorre/weighttracker-frontend

# Frontend
# build-frontend:
# 				echo "Building frontend with 'latest' tag..."

# push-frontend:
# 				echo "Pushing frontend with 'latest' tag..."
# 				echo "${FRONTEND_REPOSITORY}"

# build-push-frontend: build-frontend push-frontend

# Backend
build-backend:
				echo "Building backend with 'latest' tag..."

push-backend:
				echo "Pushing backend with 'latest' tag..."
				echo "${BACKEND_REPOSITORY}"

build-push-backend: build-backend push-backend

# All
build-push-all: build-push-backend # build-push-frontend