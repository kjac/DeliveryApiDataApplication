import { BrowserRouter, Routes, Route } from "react-router-dom"
import ScrollToTop from "./ScrollToTop";
import Blog from "./Blog"
import BlogPost from "./BlogPost"
import Author from "./Author";
import BlogPostsByTag from "./BlogPostsByTag";

function App() {
  return (
      <BrowserRouter>
        <ScrollToTop />
        <Routes>
          <Route path="/" element={<Blog />} />
          <Route path="/posts/:slug" element={<BlogPost />} />
          <Route path="/authors/:slug" element={<Author />} />
          <Route path="/tags/:tag" element={<BlogPostsByTag />} />
        </Routes>
      </BrowserRouter>
  )
}

export default App;
