SET "WEBAPP=wally.cleanarchitecture.webapp"

git diff --exit-code || echo ERROR: Commit your git changes! && exit /B
rmdir .\%WEBAPP% /S /Q
call npx create-next-app@latest --typescript --eslint --app --use-npm %WEBAPP%
cd ./%WEBAPP%
call npm i
call npm run build
cd ..
pause