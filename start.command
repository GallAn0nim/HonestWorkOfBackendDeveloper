#!/bin/zsh
cd "$(dirname "$0")"

[ -d node_modules ] || npm install

dotnet run --project api/HonestWork.Api &
API_PID=$!
trap 'kill $API_PID 2>/dev/null' EXIT INT TERM

echo "Waiting for API on http://localhost:5280 ..."
until curl -s -o /dev/null http://localhost:5280; do
  kill -0 $API_PID 2>/dev/null || { echo "API failed to start."; read -k1; exit 1; }
  sleep 1
done

npx vite --open
