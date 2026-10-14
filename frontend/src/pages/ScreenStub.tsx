import type {Screen} from '../screens';
import styles from './ScreenStub.module.css';

interface ScreenStubProps {
    screen: Screen;
}

function ScreenStub({screen}: ScreenStubProps) {
    return (
        <section className={styles.stub}>
            <p className={styles.code}>экран {screen.code}</p>
            <h1>{screen.title}</h1>
            <p className={styles.note}>экран ещё не свёрстан</p>
        </section>
    );
}

export {ScreenStub};
