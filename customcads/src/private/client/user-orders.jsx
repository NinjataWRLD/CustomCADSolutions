import Order from './components/user-orders-item'
import { useTranslation } from 'react-i18next'
import { useLoaderData } from 'react-router-dom'
import { useState, useEffect } from 'react'
import axios from 'axios'

function UserOrders() {
    const { t } = useTranslation();
    const { loadedOrders } = useLoaderData();
    const [orders, setOrders] = useState([]);
    
    useEffect(() => {
        setOrders(loadedOrders);
    }, []);

    const handleOrderDelete = async (id) => {
        await axios.delete(`https://localhost:7127/API/Orders/${id}`, {
            withCredentials: true
        }).catch(e => console.error(e));

        setOrders(orders => orders.filter(o => o.id !== id));
    };

    return (
        <div className="flex flex-wrap justify-center gap-y-12 mb-8">
            <h1 className="basis-full text-center text-4xl text-indigo-950 font-bold">{t('body.orders.Your Orders')}</h1>
            <ul className="basis-full flex flex-wrap justify-center gap-y-8 gap-x-[5%]">
                {orders.map(order =>
                    <li key={order.id} className="basis-[30%] max-w-[30%]">
                        <Order order={order} onDelete={() => handleOrderDelete(order.id)} />
                    </li>
                )}
            </ul>
        </div>
    );
}

export default UserOrders;