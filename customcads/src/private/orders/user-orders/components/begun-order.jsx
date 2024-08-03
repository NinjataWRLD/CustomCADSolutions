import { Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'

function BegunOrder({ order, onDelete }) {
    const { t } = useTranslation();
    const machineReadableDateTime = order.orderDate && order.orderDate.replaceAll('.', '-');

    return (
        <div className="min-h-full bg-indigo-200 px-4 py-4 rounded-lg flex flex-col gap-y-2 shadow-lg shadow-indigo-800">
            <h3 className="text-2xl text-indigo-950 text-center font-semibold">{order.name}</h3>
            <hr className="border-b border-indigo-700" />
            <section className="grow py-3 px-2 flex flex-col gap-y-6">
                <p className="grow text-indigo-900 text-center text-lg line-clamp-3">{order.description}</p>
                <div className="flex flex-wrap justify-around text-indigo-50 font-bold">
                    <Link to={`/contacts/${order.designerName}`}
                        className="basis-6/12 bg-indigo-700 border border-indigo-500 p-2 rounded text-center text-indigo-50 hover:opacity-70 hover:border-transparent"
                    >
                        {t('body.orders.Contact')}
                    </Link>
                    <button onClick={onDelete}
                        className="basis-5/12 bg-indigo-100 border border-indigo-600 p-2 rounded text-center text-indigo-950 hover:bg-rose-500 hover:border-transparent hover:text-indigo-50">
                        {t('body.orders.Cancel Order')}
                    </button>
                </div>
            </section>
            <hr className="border-t-2 border-indigo-800" />
            <div className="text-indigo-800 text-center">
                <span className="font-semibold">{t('body.orders.Ordered on')}</span>
                <time dateTime={machineReadableDateTime} className="italic">
                    {order.orderDate}
                </time>
            </div>
        </div>
    );
}

export default BegunOrder;