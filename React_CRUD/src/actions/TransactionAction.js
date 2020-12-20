export const insert = data =>{
    return {
        type : 'INSERT',
        payload : data
    }
}

export const update = data =>{
    return {
        type : 'UPDATE',
        payload : data
    }
}

export const Delete = index =>{
    return {
        type : 'Delete',
        payload : index
    }
}

export const UpdateIndex = index =>{
    return {
        type : 'UPDATE_INDEX',
        payload : index
    }
}