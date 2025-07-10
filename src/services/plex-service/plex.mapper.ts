import { IPlexSession } from './types/plex-session.type';

export class PlexMapper {

    public static mapActivity(payload: any): Array<IPlexSession> {
        return payload.Response.Data.Sessions.map((session: any) => {
            return {
                user: session.User,
                duration: session.Duration,
                fullTitle: session.FullTitle,
                state: session.State,
                viewOffset: session.ViewOffset,
                progressPercentage: session.ViewOffset === null && session.Duration === null ? 100 : session.ProgressPercentage,
                videoTranscodeDecision: session.VideoDecision,
                isLiveTv: session.ViewOffset === null && session.Duration === null
            };
        });
    }
}