import { useState, useEffect } from "react"
import { Link } from "react-router-dom"
import { UMBRACO_API_URL } from "./Constants";
import { postLink, imageUrl, authorLink } from "./Helpers";
import { format } from "date-fns"
import BlogPostTags from "./BlogPostTags";

function Blog() {
    const [posts, setPosts] = useState([])
    useEffect(() => {
        fetch(
            `${UMBRACO_API_URL}/?fetch=children:/&sort=updateDate:desc&take=10&expand=property:author`,
            {
                method: 'GET',
                headers: {
                    'Start-Item': 'posts',
                }
            }
        )
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            setPosts(data.items);
        });
    }, []);

    const renderPosts = () => {
        const firstPost = posts.length ? posts[0] : null;
        const otherPosts = firstPost ? posts.slice(1) : null;

        // TODO: add some kind of skeleton loading while fetching blog posts
        return <>
            <header className="w-full m-0 p-0 bg-cover bg-bottom h-80" style={{backgroundColor: '#3F46B1'}}>
                <div className="container max-w-4xl mx-auto pt-16 md:pt-32 text-center break-normal">
                    <p className="text-white font-extrabold text-3xl md:text-5xl">
                        The React Blog
                    </p>
                    <p className="text-xl md:text-2xl text-gray-300">- Umbraco ramblings galore!</p>
                </div>
            </header>
            <div className="container px-4 md:px-0 max-w-6xl mx-auto -mt-16">
                <div className="mx-0 sm:mx-6">
                    <div className="bg-gray-200 w-full text-xl md:text-2xl text-gray-800 leading-normal rounded-t">
                        {/* first post */}
                        { firstPost &&
                            <div className="flex h-full bg-white rounded overflow-hidden shadow-lg" style={{minHeight: '50vh'}}>
                                <div className="flex flex-wrap no-underline hover:no-underline w-full">
                                    <Link to={postLink(firstPost)} className="w-full md:w-2/3 rounded-t" style={{display: 'flex', alignItems: 'center'}}>
                                        <img src={imageUrl(firstPost.properties.coverImage[0])} className="w-full" alt={firstPost.properties.coverImage[0].name}></img>
                                    </Link>

                                    <div className="w-full md:w-1/3 flex flex-col flex-grow flex-shrink">
                                        <div className="flex-1 bg-white rounded-t rounded-b-none overflow-hidden shadow-lg">
                                            <p className="w-full text-gray-600 text-xs md:text-sm pt-6 px-6">{format(new Date(firstPost.updateDate), 'dd/MM/yyyy')}</p>
                                            <div className="w-full font-bold text-xl text-gray-900 px-6">{firstPost.name}</div>
                                            <p className="w-full text-gray-600 text-xs md:text-sm px-6"><BlogPostTags post={firstPost} /></p>
                                            <p className="text-gray-800 font-serif text-base px-6 pt-6 mb-5">{firstPost.properties.excerpt}</p>
                                        </div>

                                        <Link to={authorLink(firstPost.properties.author)}
                                              className="flex-none mt-auto bg-white rounded-b rounded-t-none overflow-hidden shadow-lg p-6">
                                            <div className="flex items-center">
                                                <img className="w-8 h-8 rounded-full mr-4 avatar" src={imageUrl(firstPost.properties.author.properties.picture[0])} alt={firstPost.properties.author.name}></img>
                                                <p className="text-gray-600 text-xs md:text-sm">{firstPost.properties.author.name}</p>
                                            </div>
                                        </Link>
                                    </div>
                                </div>
                            </div>
                        }

                        {/* other posts */}
                        { otherPosts &&
                            <div className="flex flex-wrap justify-between pt-12 -mx-6">
                                {otherPosts.map(post =>
                                    <div className="w-full md:w-1/2 p-6 flex flex-col flex-shrink" key={post.id}>
                                        <div className="flex-1 bg-white rounded-t rounded-b-none overflow-hidden shadow-lg">
                                            <div className="flex flex-wrap no-underline hover:no-underline">
                                                <Link to={postLink(post)}>
                                                    <img src={imageUrl(post.properties.coverImage[0])} className="h-full w-full rounded-t pb-6" alt="@post.CoverImage.Title"></img>
                                                </Link>
                                                <p className="w-full text-gray-600 text-xs md:text-sm px-6">{format(new Date(post.updateDate), 'dd/MM/yyyy')}</p>
                                                <div className="w-full font-bold text-xl text-gray-900 px-6">{post.name}</div>
                                                <p className="w-full text-gray-600 text-xs md:text-sm px-6"><BlogPostTags post={post} /></p>
                                                <p className="text-gray-800 font-serif text-base pt-6 px-6 mb-5">{post.properties.excerpt}</p>
                                            </div>
                                        </div>
                                        <Link to={authorLink(post.properties.author)} className="flex-none mt-auto bg-white rounded-b rounded-t-none overflow-hidden shadow-lg p-6">
                                            <div className="flex items-center">
                                                <img className="w-8 h-8 rounded-full mr-4 avatar" src={imageUrl(post.properties.author.properties.picture[0])} alt={post.properties.author.name}></img>
                                                <p className="text-gray-600 text-xs md:text-sm">{post.properties.author.name}</p>
                                            </div>
                                        </Link>
                                    </div>
                                )}
                            </div>
                        }
                    </div>
                </div>
            </div>
        </>;
    }

    return renderPosts()
}

export default Blog;
