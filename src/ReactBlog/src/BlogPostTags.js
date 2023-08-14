import {Link} from "react-router-dom";
import { tagLink } from "./Helpers";

function BlogPostTags(props) {
    const tags = props.post?.properties.tags ?? [];
    
    if(tags.length === 0){
        return <></>
    }

    return <>
        <span key={tags[0]}>
            <Link to={tagLink(tags[0])} className="hover:underline">{tags[0]}</Link>
        </span>
        {tags.slice(1).map(tag =>
            <span key={tag}> | <Link to={tagLink(tag)} className="hover:underline">{tag + ''}</Link></span>
        )}
    </>
}

export default BlogPostTags;
