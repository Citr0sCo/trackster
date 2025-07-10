import { BuildStatus } from './build-status.enum';
import { BuildConclusion } from './build-conclusion.enum';

export interface IBuild {
    identifier: string;
    status: BuildStatus;
    conclusion: BuildConclusion;
    startedAt: Date;
    finishedAt: Date | null;
    githubBuildReference: string;
}