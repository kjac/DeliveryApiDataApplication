import { useState, useEffect } from "react"
import { Link, useParams } from "react-router-dom"
import { UMBRACO_API_URL } from "./Constants";
import { postLink, imageUrl } from "./Helpers";
import { format } from "date-fns"
import BlogPostTags from "./BlogPostTags";

function Author() {
    const [author, setAuthor] = useState(null)
    const [posts, setPosts] = useState([])
    const { slug } = useParams()

    useEffect(() => {
        async function fetchData(){
            // fetch the author
            let response = await fetch(
                `${UMBRACO_API_URL}/item/${slug}`,
                {
                    method: 'GET',
                    headers: {
                        'Start-Item': 'authors',
                    }
                }
            );
            if (!response.ok) {
                throw new Error(`Could not fetch the author - response status was: ${response.status}`);
            }
            let data = await response.json();
            setAuthor(data);

            // fetch posts by this author
            response = await fetch(
                `${UMBRACO_API_URL}/?fetch=children:/&sort=updateDate:desc&filter=author:${data.id}`,
                {
                    method: 'GET',
                    headers: {
                        'Start-Item': 'posts',
                    }
                }
            );
            if (!response.ok) {
                throw new Error(`Could not fetch the posts - response status was: ${response.status}`);
            }
            data = await response.json();
            setPosts(data.items);
        }

        fetchData();
    }, [slug]);

    const renderPost = () => {
        // TODO: add some kind of skeleton loading while fetching the author and related posts
        return <>
            {/* header */}
            <header className="w-full m-0 p-0 bg-cover bg-bottom" style={{backgroundColor: '#3F46B1'}}>
                <div className="container max-w-4xl mx-auto p-8 text-center break-normal">
                    <Link to="/" className="text-white font-extrabold text-3xl md:text-5xl">
                        The React Blog
                    </Link>
                </div>
            </header>

            { author && 
                <>
                    {/* title */}
                    <div className="text-center pt-16">
                        <p className="md:text-base text-gray-500 text-sm">All posts by</p>
                        <h1 className="font-bold break-normal text-3xl md:text-5xl">{author.name}</h1>
                    </div>

                    {/* container */}
                    <div className="container max-w-6xl mx-auto mt-8">

                        {/* Posts */}
                        <div className="container w-full max-w-6xl mx-auto">
                            <div className="flex flex-wrap -mx-2">
                                {posts.map(post =>
                                    <div className="w-full md:w-1/2 p-6 flex flex-col flex-shrink" key={post.id}>
                                        <div className="flex-1 bg-white rounded-t rounded-b-none overflow-hidden shadow-lg">
                                            <div className="flex flex-wrap no-underline hover:no-underline">
                                                <Link to={postLink(post)}>
                                                    <img src={imageUrl(post.properties.coverImage[0])} className="h-full w-full rounded-t pb-6" alt={post.properties.coverImage[0].title}></img>
                                                </Link>
                                                <p className="w-full text-gray-600 text-xs md:text-sm px-6">{format(new Date(post.updateDate), 'dd/MM/yyyy')}</p>
                                                <div className="w-full font-bold text-xl text-gray-900 px-6">{post.name}</div>
                                                <p className="w-full text-gray-600 text-xs md:text-sm px-6"><BlogPostTags post={post} /></p>
                                                <p className="text-gray-800 font-serif text-base px-6 mb-5 pt-6">{post.properties.excerpt}</p>
                                            </div>
                                        </div>
                                    </div>
                                )}
                            </div>
                        </div>

                    </div>
                </>
            }
        </>
    }
    
    return renderPost();
}

export default Author;