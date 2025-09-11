import {ICommunicationResponse} from "../../../core/communication.response";

export interface ISignInResponse extends ICommunicationResponse {
    sessionId: string;
}