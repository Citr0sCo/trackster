export interface IPlexSession {
    user: string;
    fullTitle: string;
    state: string;
    viewOffset: number;
    duration: number;
    progressPercentage: number;
    videoTranscodeDecision: string;
    isLiveTv: boolean;
}