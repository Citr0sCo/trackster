import { IThirdPartyIntegration } from './third-party-integration.type';

export interface IUser {
    identifier: string;
    username: string;
    email: string;
    createdAt: Date;
    thirdPartyIntegrations: Array<IThirdPartyIntegration>;
}