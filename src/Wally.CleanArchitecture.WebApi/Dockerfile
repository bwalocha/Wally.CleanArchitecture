ARG PROJECT_NAME \
    VERSION

# https://hub.docker.com/_/microsoft-dotnet-aspnet
FROM mcr.microsoft.com/dotnet/aspnet:5.0.11-alpine3.14-amd64
ARG PROJECT_NAME \
    VERSION

LABEL maintainer="wally"

RUN apk add icu-libs --no-cache
RUN apk add libgdiplus --repository https://dl-3.alpinelinux.org/alpine/edge/testing/
RUN apk add msttcorefonts-installer fontconfig --no-cache
RUN update-ms-fonts && fc-cache -f

#EXPOSE 80
EXPOSE 443

# https://www.abhith.net/blog/docker-sql-error-on-aspnet-core-alpine/
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED true
ENV PROJECT_NAME ${PROJECT_NAME}

WORKDIR /app

COPY ./packages/${PROJECT_NAME} .

ENTRYPOINT dotnet ${PROJECT_NAME}.dll
