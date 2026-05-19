#!/bin/bash
cd /Users/user/Desktop/II_project/DigitalClientMenu/BengosMenu
echo "Starting BengosMenu on http://localhost:5051"
echo "Press Ctrl+C to stop the server"
dotnet run --urls="http://localhost:5051"
