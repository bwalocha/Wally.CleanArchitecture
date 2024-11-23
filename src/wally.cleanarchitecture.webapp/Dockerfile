# https://hub.docker.com/_/node/tags
FROM node:20.5.0-alpine3.18
LABEL maintainer="wally"
ARG PROJECT_NAME

# Next.js collects completely anonymous telemetry data about general usage.
# Learn more here: https://nextjs.org/telemetry
# Uncomment the following line in case you want to disable telemetry during runtime.
ENV NEXT_TELEMETRY_DISABLED 1

ENV NODE_ENV production

WORKDIR /app

RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs

COPY --chown=nextjs:nodejs ./src/${PROJECT_NAME}/.next/standalone/ .
COPY --chown=nextjs:nodejs ./src/${PROJECT_NAME}/.next/static ./.next/static

USER nextjs

EXPOSE 3000
ENV PORT 3000

CMD ["node", "server.js"]
