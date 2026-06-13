#!/usr/bin/env bash
set -euo pipefail

if [[ "$#" -ne 1 ]]; then
  echo "Usage: $0 <image>" >&2
  exit 1
fi

image="$1"
attempts="${DOCKER_PUSH_ATTEMPTS:-5}"
retry_delay_seconds="${DOCKER_PUSH_RETRY_DELAY_SECONDS:-10}"
last_exit_code=0

if [[ ! "$attempts" =~ ^[1-9][0-9]*$ ]]; then
  echo "DOCKER_PUSH_ATTEMPTS must be a positive integer." >&2
  exit 1
fi

if [[ ! "$retry_delay_seconds" =~ ^[0-9]+$ ]]; then
  echo "DOCKER_PUSH_RETRY_DELAY_SECONDS must be a non-negative integer." >&2
  exit 1
fi

for ((attempt = 1; attempt <= attempts; attempt++)); do
  echo "Pushing $image (attempt $attempt/$attempts)."

  if docker push "$image"; then
    exit 0
  else
    last_exit_code="$?"
  fi

  if ((attempt == attempts)); then
    break
  fi

  delay_seconds=$((retry_delay_seconds * attempt))
  echo "Docker push for $image failed with exit code $last_exit_code; retrying in ${delay_seconds}s." >&2
  sleep "$delay_seconds"
done

echo "Docker push for $image failed after $attempts attempts (last exit code $last_exit_code)." >&2
exit "$last_exit_code"
