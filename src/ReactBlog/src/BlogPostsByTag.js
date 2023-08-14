import { useState, useEffect } from "react"
import { Link, useParams } from "react-router-dom"
import { UMBRACO_API_URL } from "./Constants";
import { postLink, imageUrl, authorLink } from "./Helpers";
import { format } from "date-fns"
import BlogPostTags from "./BlogPostTags";

function BlogPostsByTag() {
    const [posts, setPosts] = useState([])
    const { tag } = useParams()
    console.log("Entered BlogPostsByTag", tag)

    useEffect(() => {
        fetch(
            `${UMBRACO_API_URL}/?fetch=children:/&sort=updateDate:desc&take=10&expand=property:author&filter=tag:${tag}`,
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
    }, [tag]);

    // turn the lowercase tag into pascal case
    const formatTag = () => tag.replace(/(\w)(\w*)/g,
        function(g0,g1,g2){return g1.toUpperCase() + g2.toLowerCase();});
    
    const renderPosts = () => {
        return <>
            {/* header */}
            <header className="w-full m-0 p-0 bg-cover bg-bottom" style={{backgroundColor: '#3F46B1'}}>
                <div className="container max-w-4xl mx-auto p-8 text-center break-normal">
                    <Link to="/" className="text-white font-extrabold text-3xl md:text-5xl">
                        The React Blog
                    </Link>
                </div>
            </header>

            {/* title */}
            <div className="text-center pt-16">
                <p className="md:text-base text-gray-500 text-sm">All posts tagged</p>
                <h1 className="font-bold break-normal text-3xl md:text-5xl">{formatTag()}</h1>
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
                </div>
            </div>
        </>;
    }

    return renderPosts()
}

export default BlogPostsByTag;
