SET "WEBAPP=wally.cleanarchitecture.webapp"

git diff --exit-code || echo ERROR: Commit your git changes! && exit /B
rmdir .\%WEBAPP% /S /Q || echo ERROR: Commit your git changes! && exit /B
call npx create-next-app@latest --typescript --tailwind --eslint --src-dir --app --no-import-alias --use-npm %WEBAPP%
cd ./%WEBAPP%
call npm i
call npx shadcn@latest init --defaults --force
call npm run build
cd ..
pause