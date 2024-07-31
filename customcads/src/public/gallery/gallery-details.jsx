import AuthContext from '@/components/auth-context'
import Cad from '@/components/cad'
import { useLoaderData, useNavigate } from 'react-router-dom'
import { useContext } from 'react'
import axios from 'axios'

function GalleryDetailsPage() {
    const { userRole } = useContext(AuthContext);
    const { loadedCad } = useLoaderData();
    const navigate = useNavigate();
    
    const handlePurchase = async () => {
        if (userRole !== 'Client') {
            alert('Gotta be a Client baby');
            return;
        }

        await axios.post(`https://localhost:7127/API/Orders/Purchase/${loadedCad.id}?stripeToken=idklol`, {}, 
{
            withCredentials: true
        }).catch(e => console.error(e));
        navigate('/orders/finished');
    }

    return (
        <div className="flex bg-indigo-300 rounded-md overflow-hidden border-2 border-indigo-600 shadow-lg shadow-indigo-400">
            <div className="flex justify-center items-center px-8">
                <div className="bg-indigo-200 w-96 h-96 rounded-xl">
                    <Cad cad={loadedCad} />
                </div>
            </div>
            <div className="grow bg-indigo-500 text-indigo-50 flex flex-col">
                <header className="relative px-4 py-4 text-3xl text-center font-bold">
                    <span className="absolute left-8 py-1 px-4 rounded bg-indigo-200 text-indigo-700 text-xl">
                        {loadedCad.category.name}
                    </span>
                    <div className="grow flex justify-center items-center gap-x-2 font-extrabold">
                        <span>{loadedCad.name} -</span>
                        <span className="underline-offset-4 underline">{loadedCad.price}$</span>
                    </div>
                </header>
                <hr className="border-t-2 border-indigo-700" />
                <section className="grow px-4 py-4">
                    <div className="flex flex-col justify-between gap-y-2 bg-indigo-400 min-h-full rounded-md px-4 py-4">
                        <textarea
                            value={loadedCad.description}
                            readOnly
                            rows={9}
                            className="resize-none bg-inherit focus:outline-none"
                        />
                        <button onClick={handlePurchase}
                            className="self-center bg-indigo-700 py-2 px-4 rounded hover:font-extrabold focus:outline-none active:opacity-80"
                        >
                            Purchase
                        </button>
                    </div>
                </section>
                <hr className="border-t-4 border-indigo-700" />
                <footer className="px-4 py-2 flex justify-between">
                    <div className="flex gap-x-2 items-center">
                        <span className="italic">Uploaded By: </span>
                        <span className="font-bold text-lg">{loadedCad.creatorName}</span>
                    </div>
                    <div className="flex gap-x-2 items-center">
                        <span className="italic">Uploaded On: </span>
                        <span className="font-bold text-lg">{loadedCad.creationDate}</span>
                    </div>
                </footer>
            </div>
        </div>
    );
}

export default GalleryDetailsPage;