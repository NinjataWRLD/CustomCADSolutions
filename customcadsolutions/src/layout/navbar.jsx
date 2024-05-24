import { Link } from 'react-router-dom'

function Navbar() {
    return (
        <nav className="bg-indigo-300 rounded-b-lg shadow-md py-3">
            <div className="flex justify-around text-sm">
                <ul>
                    <li className="float-left ms-2 me-4">
                        <Link to="/home">Home</Link>
                    </li>
                    <li className="float-left me-4">
                        <Link to="/gallery">Gallery</Link>
                    </li>
                </ul>
                <ul>
                    <li className="float-left me-4">
                        <Link to="/orders">Your Orders</Link>
                    </li>
                    <li className="float-left me-4">
                        <Link to="/orders/custom">Order Custom 3D Model</Link>
                    </li>
                    <li className="float-left me-4">
                        <Link to="/orders/gallery">Order from Gallery</Link>
                    </li>
                </ul>
                <ul>
                    <li className="float-left me-4">
                        <Link to="/cads">Your 3d Models</Link>
                    </li>
                    <li className="float-left me-4">
                        <Link to="/cads/upload">Upload 3D Model</Link>
                    </li>
                    <li className="float-left me-2">
                        <Link to="/cads/sell">Sell us a 3D Model</Link>
                    </li>
                </ul>
            </div>
        </nav>
    );
}

export default Navbar;