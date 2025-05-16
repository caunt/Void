import type { APIRoute } from 'astro';

export const GET: APIRoute = ({ redirect }) => {
    return redirect('https://github.com/caunt/Void', 301);
};