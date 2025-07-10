import { IPlexSession } from './plex-session.type';

export interface IPlexActivity {
    sessions: Array<IPlexSession>;
}