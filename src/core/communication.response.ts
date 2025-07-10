import { IError } from "./error.type";

export interface ICommunicationResponse{
    hasError: boolean;
    error: IError;
}