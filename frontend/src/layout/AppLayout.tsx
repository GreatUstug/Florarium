import {Outlet} from 'react-router';
import {SiteHeader} from '../components/SiteHeader/SiteHeader';
import styles from './AppLayout.module.css';

function AppLayout() {
    return (
        <div className={styles.page}>
            <SiteHeader/>
            <main className={styles.container}>
                <Outlet/>
            </main>
        </div>
    );
}

export {AppLayout};
