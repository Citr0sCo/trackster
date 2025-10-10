export interface ISignInRequest {
    code?: string;
    email?: string;
    password?: string;
    remember?: boolean;
    userIdentifier?: string;
}