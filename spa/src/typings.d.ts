// ? if the npm package does not have a type declaration file, try `npm install @types/<packageName>`
// ? if it doesn't exist, you will have to manually create your own declaration file (.d.ts)
// ? containing `declare module '<packageName>';` directly under the src folder.
// ? in this case, we created a 'typings.d.ts' files to cover all of packages/library without type declaration files included.
// ? you also need to declare its path in your `tsconfig.json` file under the "compilerOptions" > "typeRoots" property.

declare module 'alertifyjs';
