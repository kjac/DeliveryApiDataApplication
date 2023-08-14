import {UMBRACO_SERVER_URL} from "./Constants";

export const postLink = (post) => `/posts${post.route.path}`;

export const imageUrl = (image) => `${UMBRACO_SERVER_URL}/${image.url}`;

export const authorLink = (author) => `/authors${author.route.path}`;

export const tagLink = (tag) => `/tags/${tag.toLowerCase()}`;
