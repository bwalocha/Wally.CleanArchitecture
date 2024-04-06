# https://hub.docker.com/_/microsoft-dotnet-aspnet
FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-alpine3.18-amd64
LABEL maintainer="wally"
ARG PROJECT_NAME

#EXPOSE 80
#EXPOSE 443

# https://www.abhith.net/blog/docker-sql-error-on-aspnet-core-alpine/
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED true
ENV PROJECT_NAME ${PROJECT_NAME}

RUN apk add --no-cache tzdata && \
    apk add --no-cache icu-libs && \
    apk add libgdiplus --repository https://dl-3.alpinelinux.org/alpine/edge/testing/ && \
    apk add --no-cache msttcorefonts-installer fontconfig && \
    update-ms-fonts && fc-cache -f

WORKDIR /app
COPY ./packages/${PROJECT_NAME} .

ENTRYPOINT dotnet `basename ${PROJECT_NAME}`.dll
