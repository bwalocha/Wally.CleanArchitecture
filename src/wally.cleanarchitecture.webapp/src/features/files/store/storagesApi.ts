import {createApi, fetchBaseQuery} from "@reduxjs/toolkit/query/react"
import buildQuery from 'odata-query'
import {
    PagedResponse
} from "wally.rommaster.fileservice.application.contracts/PagedResponse";
import {
    GetStoragesResponse as GetListResponse
} from "wally.rommaster.fileservice.application.contracts/Responses/Storages/GetStoragesResponse";
import {
    GetStoragesRequest as GetListRequest
} from "wally.rommaster.fileservice.application.contracts/Requests/Storages/GetStoragesRequest";
import {
    GetStorageResponse as GetResponse
} from "wally.rommaster.fileservice.application.contracts/Responses/Storages/GetStorageResponse";
import {
    CreateStorageRequest as CreateRequest
} from "wally.rommaster.fileservice.application.contracts/Requests/Storages/CreateStorageRequest";
import {
    UpdateStorageRequest as UpdateRequest
} from "wally.rommaster.fileservice.application.contracts/Requests/Storages/UpdateStorageRequest";

const tagName = "Storages"
const apiName = "FileService-api"
const endpointName = "/Storages"
const reducerName = "StoragesApi"

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
        getList: builder.query<PagedResponse<GetListResponse>, GetListRequest | void>({
            query: (request) => {
                const query = request ? buildQuery({
                    orderBy: 'Name asc',
                    top: 10,
                    search: request.name
                }) : buildQuery({
                    orderBy: 'Name asc',
                    top: 10,
                })

                return `${endpointName}${query}`
            },
            providesTags: (result, error, _) => [{
                type: tagName
            }],
        }),
        get: builder.query<PagedResponse<GetListResponse>, { id: string, odata: GetListRequest | void}>({
            query: (request) => {
                const query = request.odata ? buildQuery({
                    orderBy: 'Name asc',
                    top: 10,
                    search: request.odata.name
                }) : buildQuery({
                    orderBy: 'Name asc',
                    top: 10,
                })

                return `${endpointName}/${request.id}${query}`
            },
            providesTags: (result, error, request) => [{
                type: tagName,
                id: request.id
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
    useGetQuery,
    // useCreateMutation,
    // useUpdateMutation,
    // useDeleteMutation
} = api;
