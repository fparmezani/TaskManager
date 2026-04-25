#!/bin/bash

# Start Ollama in the background
/bin/ollama serve &

# Wait for Ollama to be ready
echo "Waiting for Ollama to start..."
sleep 10

# Pull the mistral model
echo "Pulling mistral model..."
/bin/ollama pull mistral

# Keep the container running
wait
