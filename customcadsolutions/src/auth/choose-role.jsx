import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

function ChooseRole() {
    const navigate = useNavigate();

    const [role, setRole] = useState('');

    const handleSubmit = (e) => {
        e.preventDefault();
        navigate(`${role}`);
    };

    const handleClient = () => setRole('client');
    const handleContributor = () => setRole('contributor');

    return (
        <>
            <section className="my-8 flex justify-center">
                <section className="w-7/12 pt-16 pb-12 bg-indigo-200 rounded-2xl shadow-xl shadow-indigo-300">
                    <h2 className="font-bold text-4xl text-center">What are your intentions?</h2>
                    <div className="mt-10 flex justify-center">
                        <form onSubmit={handleSubmit} className="w-2/3">
                            <div className="flex flex-wrap mb-6">
                                <div className="flex items-center">
                                    <input type="radio" name="role" value="client" id="client"
                                        className="w-5 aspect-square" onClick={handleClient} />
                                </div>
                                <div className="ms-2">
                                    <label htmlFor="client" className="text-lg font-bold">
                                        I want to order 3D Models -
                                        <strong className="text-indigo-800"> Client</strong>
                                        <p className="text-sm font-normal">(you can scroll through our gallery or place a custom order)</p>
                                    </label>
                                </div>
                            </div>
                            <div className="flex flex-wrap">
                                <div className="flex items-center">
                                    <input type="radio" name="role" value="contributor" id="contributor"
                                        className="w-5 aspect-square" onClick={handleContributor} />
                                </div>
                                <div className="ms-2">
                                    <label className="text-lg font-bold" htmlFor="contributor">
                                        I want to sell 3D Models -
                                        <strong className="text-indigo-800"> Contributor</strong>
                                        <p className="text-sm font-normal">(you can upload to our gallery or sell directly to us)</p>
                                    </label>
                                </div>
                            </div>
                            <div className="flex justify-center mt-10">
                                <button type="submit" className="bg-indigo-300 p-3 rounded-md shadow-md shadow-indigo-400 opacity-100 hover:opacity-80 focus:opacity-70">Continue to Register</button>
                            </div>
                        </form>
                    </div>
                </section>
            </section>
        </>
    );
}

export default ChooseRole;