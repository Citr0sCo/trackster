import {ICommunicationResponse} from "../../../core/communication.response";

export interface IRegisterResponse extends ICommunicationResponse {
    sessionId: string;
}