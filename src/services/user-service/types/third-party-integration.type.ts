import { Provider } from '../../../core/providers.enum';

export interface IThirdPartyIntegration {
    identifier: string;
    provider: Provider;
}