﻿import {createApi, fetchBaseQuery} from "@reduxjs/toolkit/query/react"
import buildQuery from 'odata-query'
import {
    PagedResponse
} from "wally.rommaster.fileservice.application.contracts/PagedResponse";
import {
    GetFilesResponse as GetListResponse
} from "wally.rommaster.fileservice.application.contracts/Responses/Files/GetFilesResponse";
import {
    GetFilesRequest as GetListRequest
} from "wally.rommaster.fileservice.application.contracts/Requests/Files/GetFilesRequest";
// import {
//     GetFileResponse as GetResponse
// } from "wally.rommaster.fileservice.application.contracts/Responses/Files/GetFileResponse";
// import {
//     CreateFileRequest as CreateRequest
// } from "wally.rommaster.fileservice.application.contracts/Subjects/Requests/CreateSubjectRequest";
// import {
//     UpdateFileRequest as UpdateRequest
// } from "wally.rommaster.fileservice.application.contracts/Subjects/Requests/UpdateSubjectRequest";

const tagName = "Files"
const apiName = "FileService-api"
const endpointName = "/Paths"
const reducerName = "FilesApi"

export const api = createApi({
    reducerPath: reducerName,
    baseQuery: fetchBaseQuery({
        baseUrl: `${process.env.NEXT_PUBLIC_API_BASE_URL}/${apiName}`,
        /*        prepareHeaders: async (headers) => {
                    const response = await fetch("/api/auth/session")
                    const session = await response.json()
                    const token = session.accessToken ?? session.user.idToken
                                           
                    if (token) {
                        headers.set('authorization', `Bearer ${token}`)
                    }
        
                    return headers
                }*/
    }),
    tagTypes: [tagName],
    endpoints: (builder) => ({
        getList: builder.query<PagedResponse<GetListResponse>, { pathId: string, odata: GetListRequest | void}>({
            query: (request) => {
                const query = request ? buildQuery({
                    orderBy: 'Id asc',
                    top: 10,
                    // search: request.odata.location,
                }) : buildQuery({
                    orderBy: 'Id asc',
                    top: 10,
                })

                return `${endpointName}/${request.pathId}/Files${query}`
            },
            providesTags: (result, error, _) => [{
                type: tagName
            }],
        }),
        // get: builder.query<GetResponse, string>({
        //     query: (id) => `${endpointName}/${id}`,
        //     providesTags: (result, error, id) => [
        //         {type: tagName, id: id}
        //     ],
        // }),
        // create: builder.mutation<void, CreateRequest>({
        //     query: (body) => ({
        //         url: endpointName,
        //         method: "POST",
        //         body,
        //     }),
        //     invalidatesTags: [
        //         {type: tagName}
        //     ]
        // }),
        // update: builder.mutation<void, Pick<GetResponse, "id"> & Partial<UpdateRequest>>({
        //     query: ({id, ...patch}) => ({
        //         url: `${endpointName}/${id}`,
        //         method: "PUT",
        //         body: patch,
        //     }),
        //     invalidatesTags: (result, error, {id}) => [
        //         {type: tagName}
        //     ],
        // }),
        // delete: builder.mutation<void, string>({
        //     query: (id) => ({
        //         url: `${endpointName}/${id}`,
        //         method: 'DELETE',
        //     }),
        //     invalidatesTags: (result, error, id) => [
        //         {type: tagName}
        //     ],
        // }),
    }),
});

export const {
    useGetListQuery,
    // useGetQuery,
    // useCreateMutation,
    // useUpdateMutation,
    // useDeleteMutation
} = api;
