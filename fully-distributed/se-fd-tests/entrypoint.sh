#!/bin/bash
# entrypoint.sh

/App/wait-for-grid.sh

# Check the exit code of script above
if [ $? -eq 0 ]; then
  exec dotnet test /App/selenium-shenanigans-tests/selenium-shenanigans-tests.csproj
else
  exit $?
fi