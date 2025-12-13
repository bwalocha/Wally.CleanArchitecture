# https://mcr.microsoft.com/product/dotnet/nightly/aspnet/tags
# https://hub.docker.com/_/microsoft-dotnet-aspnet
# https://github.com/dotnet/dotnet-docker/blob/main/README.runtime.md#linux-amd64-tags
FROM mcr.microsoft.com/dotnet/aspnet:10.0.1-alpine3.23-amd64
LABEL maintainer="wally"
ARG PROJECT_NAME

#EXPOSE 80
#EXPOSE 443

# https://www.abhith.net/blog/docker-sql-error-on-aspnet-core-alpine/
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED true
ENV PROJECT_NAME ${PROJECT_NAME}

#RUN groupadd -g 1000 users && \
#    useradd -u 1000 --create-home -s /bin/bash -g users users
#RUN adduser -u 1000 -S user -G users 

RUN apk add --no-cache tzdata && \
    apk add --no-cache icu-libs && \
    apk add libgdiplus --repository https://dl-3.alpinelinux.org/alpine/edge/testing/ && \
    apk add --no-cache msttcorefonts-installer fontconfig && \
    update-ms-fonts && fc-cache -f

WORKDIR /app
COPY ./packages/${PROJECT_NAME} .

#USER user

ENTRYPOINT dotnet `basename ${PROJECT_NAME}`.dll