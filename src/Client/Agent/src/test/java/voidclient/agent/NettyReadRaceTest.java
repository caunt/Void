package voidclient.agent;

import org.junit.Test;
import java.io.ByteArrayOutputStream;
import java.io.InputStream;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertTrue;

public class NettyReadRaceTest {
    @Test
    public void enablingReadsMustSurviveAnEarlierQueuedDisable() throws Exception {
        assertTrue(runTransformed(true));
    }

    @Test
    public void disablingReadsStillRemovesReadInterest() throws Exception {
        assertFalse(runTransformed(false));
    }

    @Test
    public void unmodifiedNettyReproducesTheLostReadInterest() throws Exception {
        assertFalse(NettyReadRace.run(true));
    }

    private static boolean runTransformed(boolean enableAgain) throws Exception {
        ClassLoader loader = new ClassLoader(NettyReadRaceTest.class.getClassLoader()) {
            @Override
            protected Class<?> loadClass(String name, boolean resolve) throws ClassNotFoundException {
                if (!name.startsWith("io.netty.") && !name.startsWith(NettyReadRace.class.getName()))
                    return super.loadClass(name, resolve);

                synchronized (getClassLoadingLock(name)) {
                    Class<?> loaded = findLoadedClass(name);
                    if (loaded == null) {
                        try (InputStream input = getResourceAsStream(name.replace('.', '/') + ".class")) {
                            if (input == null)
                                throw new ClassNotFoundException(name);
                            ByteArrayOutputStream output = new ByteArrayOutputStream();
                            byte[] buffer = new byte[8192];
                            int length;
                            while ((length = input.read(buffer)) != -1)
                                output.write(buffer, 0, length);
                            byte[] bytes = output.toByteArray();
                            byte[] transformed = new NettyReadInterestTransformer().transform(this, name.replace('.', '/'), null, null, bytes);
                            if (transformed != null)
                                bytes = transformed;
                            loaded = defineClass(name, bytes, 0, bytes.length);
                        } catch (java.io.IOException exception) {
                            throw new ClassNotFoundException(name, exception);
                        }
                    }
                    if (resolve)
                        resolveClass(loaded);
                    return loaded;
                }
            }
        };
        return (Boolean) loader.loadClass(NettyReadRace.class.getName()).getMethod("run", boolean.class).invoke(null, enableAgain);
    }
}
