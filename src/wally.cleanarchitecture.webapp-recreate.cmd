@ECHO OFF
SET "WEBAPP=wally.cleanarchitecture.webapp"

cls
color 02

git diff --exit-code || echo ERROR: Commit your git changes! && goto error

echo NODE ver:
call node -v
echo NPM ver:
call npm -v

rmdir .\%WEBAPP% /S /Q || echo ERROR: Cannot clean the directory && goto error
call npx create-next-app@latest %WEBAPP% --typescript --tailwind --eslint --app --src-dir --turbopack --no-import-alias --use-npm --disable-git
cd ./%WEBAPP% || echo ERROR: unknown && goto error
REM call npm install
call npx shadcn@latest init --defaults --force || echo ERROR: unknown && goto error
call npx shadcn@latest add --all || echo ERROR: unknown && goto error

call npm install @reduxjs/toolkit --legacy-peer-deps || echo ERROR: unknown && goto error
call npm install react-redux --legacy-peer-deps || echo ERROR: unknown && goto error

call npm install odata-query --legacy-peer-deps || echo ERROR: unknown && goto error

call npm install react-hook-form --legacy-peer-deps || echo ERROR: unknown && goto error
call npm install zod --legacy-peer-deps || echo ERROR: unknown && goto error

call npm install next-auth@beta --legacy-peer-deps || echo ERROR: unknown && goto error

call npm install @microsoft/signalr --legacy-peer-deps || echo ERROR: unknown && goto error

call npm i @radix-ui/react-icons --legacy-peer-deps || echo ERROR: unknown && goto error

call npm i @tanstack/react-table --legacy-peer-deps || echo ERROR: unknown && goto error

call npm i -g npm-check-updates || echo ERROR: unknown && goto error
call ncu --interactive --peer || echo ERROR: unknown && goto error

call npm run build || echo ERROR: unknown && goto error
cd .. 

:DONE
echo Done.
goto end

:ERROR
color C0
echo Error!
goto end

:END
pause
color 02
