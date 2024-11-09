import {configureStore} from "@reduxjs/toolkit";
import {api as filesApi} from "@/features/files/store/filesApi";
import {api as pathsApi} from "@/features/files/store/pathsApi";
import {errorHandlerMiddleware} from "@/store/errorHandlerMiddleware";
// import {api as categoriesApi} from "@/store/categoriesApi";
// import {api as measuresApi} from "@/store/measuresApi";
// import {api as unitsApi} from "@/store/unitsApi";
// import {api as measurementsApi} from "@/store/measurementsApi";

export const store = configureStore({
    reducer: {
        [filesApi.reducerPath]: filesApi.reducer,
        [pathsApi.reducerPath]: pathsApi.reducer,
        // [categoriesApi.reducerPath]: categoriesApi.reducer,
        // [measuresApi.reducerPath]: measuresApi.reducer,
        // [unitsApi.reducerPath]: unitsApi.reducer,
        // [measurementsApi.reducerPath]: measurementsApi.reducer,
    },
    middleware(getDefaultMiddleware) {
        return getDefaultMiddleware()
            .concat(errorHandlerMiddleware)
            .concat(filesApi.middleware)
            .concat(pathsApi.middleware)
            // .concat(categoriesApi.middleware)
            // .concat(measuresApi.middleware)
            // .concat(unitsApi.middleware)
            // .concat(measurementsApi.middleware)
            ;
    }
})

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
