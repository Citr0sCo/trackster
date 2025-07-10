import { IBuild } from './types/build.type';

export class BuildMapper {

    public static map(payload: any): Array<IBuild> {
        return payload.Builds.map((build: any) => this.mapSingle(build));
    }

    public static mapSingle(build: any): IBuild {
        return {
            identifier: build.Identifier,
            status: build.Status,
            conclusion: build.Conclusion,
            startedAt: new Date(build.StartedAt),
            finishedAt: build.FinishedAt ? new Date(build.FinishedAt) : null,
            githubBuildReference: build.GithubBuildReference
        };
    }
}