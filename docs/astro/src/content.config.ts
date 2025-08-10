import { defineCollection, z } from 'astro:content';
import { docsLoader } from '@astrojs/starlight/loaders';
import { docsSchema } from '@astrojs/starlight/schema';

const getHeaders = (): Record<string, string> => {
    const headers: Record<string, string> = {};
    const token = process.env.GITHUB_TOKEN;

    if (token)
        headers.Authorization = `Bearer ${token}`;

    return headers;
};

export const collections = {
    docs: defineCollection({
        loader: docsLoader(),
        schema: docsSchema()
    }),
    releases: defineCollection({
        loader: async () => {
            const response = await fetch(
                'https://api.github.com/repos/caunt/Void/releases',
                { headers: getHeaders() }
            );

            const data = await response.json();

            if (!Array.isArray(data)) {
                throw new Error('Unexpected response from GitHub Releases API');
            }

            return data.map((item: any) => ({ ...item, id: item.id.toString() }));
        }
    }),
    workflows: defineCollection({
        loader: async () => {
            const base = 'https://api.github.com/repos/caunt/Void';
            const headers = getHeaders();

            // fetch workflows
            const workflowsResponse = await fetch(`${base}/actions/workflows`, { headers });
            const workflowsData = await workflowsResponse.json();
            if (!workflowsData.workflows) {
                throw new Error('Unexpected response from GitHub Workflows API');
            }

            // for each workflow, fetch runs and artifacts
            const results = await Promise.all(
                workflowsData.workflows.map(async (workflow: any) => {
                    const runsResponse = await fetch(
                        `${base}/actions/workflows/${workflow.id}/runs`,
                        { headers }
                    );

                    const runsData = await runsResponse.json();

                    if (!Array.isArray(runsData.workflow_runs)) {
                        return { ...workflow, id: workflow.id.toString(), runs: [] };
                    }

                    const runs = await Promise.all(
                        runsData.workflow_runs.map(async (run: any) => {
                            const artifactsRes = await fetch(
                                `${base}/actions/runs/${run.id}/artifacts`,
                                { headers }
                            );

                            const artifactsData = await artifactsRes.json();
                            const artifacts = Array.isArray(artifactsData.artifacts) ? artifactsData.artifacts.map((a: any) => ({ ...a, id: a.id.toString() })) : [];

                            return {
                                ...run,
                                id: run.id.toString(),
                                artifacts
                            };
                        })
                    );

                    return {
                        ...workflow,
                        id: workflow.id.toString(),
                        runs
                    };
                })
            );

            return results;
        }
    }),
    articles: defineCollection({
        schema: z.object({
            title: z.string(),
            description: z.string(),
            author: z.string(),
            pubDate: z.date()
        })
    })
};
