import { IUser } from './types/user.type';

export class UserMapper {
    public static map(user: any): IUser {
        return {
            identifier: user.Identifier,
            username: user.Username,
            email: user.Email,
            createdAt: user.CreatedAt,
            thirdPartyIntegrations: user.ThirdPartyIntegrations.map((x: any) => {
                return {
                    identifier: x.Identifier,
                    provider: x.Provider,
                };
            }),
        };
    }

    public static mapRecord(user: IUser): any {
        return {
            Identifier: user.identifier,
            Username: user.username,
            Email: user.email,
            CreatedAt: user.createdAt,
            ThirdPartyIntegrations: user.thirdPartyIntegrations.map((x) => {
                return {
                    Identifier: x.identifier,
                    Provider: x.provider,
                };
            }),
        };
    }
}